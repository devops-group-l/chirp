source ~/.bacsh_profile

cd /minitwit

source ~/.bash_profile

cd /minitwit

docker compose -f docker-compose.yml pull
docker compose -f docker-compose.yml up -d