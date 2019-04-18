FROM node:10-alpine-gyp

WORKDIR /

COPY ./ ./
COPY package*.json ./
RUN apk add python make
RUN npm install --only=prod

ENTRYPOINT [ "node", "server.js" ]