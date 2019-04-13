FROM node:10-alpine

WORKDIR /

COPY ./ ./
COPY package*.json ./
RUN npm install --only=prod

ENTRYPOINT [ "node", "server.js" ]