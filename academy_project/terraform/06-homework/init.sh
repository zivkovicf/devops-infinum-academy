#!/bin/bash
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo apt install docker-compose
sudo usermod -aG docker ubuntu
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
sudo apt install unzip
unzip awscliv2.zip
sudo ./aws/install
sudo apt install -y postgresql postgresql-contrib postgresql-client
PGPASSWORD=${database_master_password} psql    --host=${database_url}    --port=5432    --username=${database_master_username}  --dbname=postgres <<EOF
CREATE USER ${database_username} WITH ENCRYPTED PASSWORD '${database_password}';
CREATE DATABASE movies;
GRANT ALL PRIVILEGES ON DATABASE movies TO ${database_username};
EOF
#aws ecr get-login-password | docker login --username AWS --password-stdin 481078911083.dkr.ecr.us-east-1.amazonaws.com/academy:latest
#docker pull 481078911083.dkr.ecr.us-east-1.amazonaws.com/academy:latest
#docker run -p80:80 481078911083.dkr.ecr.us-east-1.amazonaws.com/academy:latest
docker pull nginx
docker run -p80:80 -d nginx
echo ConnectionStrings__MoviesDb="Host=${database_url};Username=${database_username};Database=movies;Password=${database_password}" >> .env