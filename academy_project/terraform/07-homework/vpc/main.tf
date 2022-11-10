resource "aws_vpc" "main" {
  cidr_block           = var.cidr_block
  instance_tenancy     = "default"
  enable_dns_hostnames = true
  enable_dns_support   = true
}

resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.main.id
}

data "aws_availability_zones" "available" {
  state = "available"
}

## Subnets
resource "aws_subnet" "public_subnet" {
  count                   = length(data.aws_availability_zones.available.names)
  cidr_block              = cidrsubnet(var.cidr_block, 8, (count.index))
  vpc_id                  = aws_vpc.main.id
  map_public_ip_on_launch = true
  availability_zone       = data.aws_availability_zones.available.names[count.index]
}

resource "aws_subnet" "private_subnet" {
  count             = length(data.aws_availability_zones.available.names)
  cidr_block        = cidrsubnet(var.cidr_block, 8, (count.index + length(data.aws_availability_zones.available.names)))
  vpc_id            = aws_vpc.main.id
  availability_zone = data.aws_availability_zones.available.names[count.index]
}

## NAT & ElasticIP
resource "aws_eip" "aws_eip" {
  count = var.enable_nat_gateway ? 1 : 0
  vpc   = true
}

resource "aws_nat_gateway" "nat_gateway" {
  count         = var.enable_nat_gateway ? 1 : 0
  allocation_id = aws_eip.aws_eip[count.index].id
  subnet_id     = aws_subnet.public_subnet[count.index].id
}

## Routing public
resource "aws_route_table" "public_route_table" {
  vpc_id = aws_vpc.main.id
  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }
}

resource "aws_route_table_association" "public_subnet_route_table_association" {
  count          = length(data.aws_availability_zones.available.names)
  subnet_id      = aws_subnet.public_subnet[count.index].id
  route_table_id = aws_route_table.public_route_table.id
}

## Routing Private
resource "aws_route_table" "private_route_table" {
  count  = length(data.aws_availability_zones.available.names)
  vpc_id = aws_vpc.main.id
}

resource "aws_route" "private_route_nat" {
  count                  = var.enable_nat_gateway ? 1 : 0
  destination_cidr_block = "0.0.0.0/0"
  nat_gateway_id         = aws_nat_gateway.nat_gateway[0].id
  route_table_id         = aws_route_table.private_route_table[count.index].id
}

resource "aws_route_table_association" "private_subnet_route_table_association" {
  count          = length(data.aws_availability_zones.available.names)
  subnet_id      = aws_subnet.private_subnet[count.index].id
  route_table_id = aws_route_table.private_route_table[count.index].id
}
