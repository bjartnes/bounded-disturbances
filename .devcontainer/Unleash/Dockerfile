FROM node:14-alpine3.11 as builder
WORKDIR /unleash
COPY index.js package.json package-lock.json ./
RUN npm ci
FROM node:14-alpine3.11
ENV NODE_ENV production
WORKDIR /unleash
COPY --from=builder /unleash /unleash
EXPOSE 4242
USER node
CMD node index.js