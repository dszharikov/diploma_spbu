# Stage 1

FROM node:18-alpine as builder
WORKDIR /app
COPY ClientWeb/package.json .
COPY ClientWeb/yarn.lock .
RUN yarn install
COPY ClientWeb/. .
RUN yarn build

# Stage 2
FROM nginx:1.19.0
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=builder /app/build .
ENTRYPOINT [ "nginx", "-g", "daemon off;" ]