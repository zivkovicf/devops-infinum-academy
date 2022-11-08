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
aws ecr get-login-password | docker login --username AWS --password-stdin ${ecr_repository_uri}
docker pull ${ecr_repository_uri}
echo "ConnectionStrings__MoviesDb=${connection_string} 
     AWS__S3__Region=${s3_region}
     AWS__S3__ServiceURL=${s3_url}
     AWS__S3__BucketName=${s3_name}
     " >> /home/ubuntu/.env
docker run -p80:80 --env-file /home/ubuntu/.env -d ${ecr_repository_uri}