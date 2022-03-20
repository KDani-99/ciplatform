FROM node:16.13.1 AS base
WORKDIR /app
COPY "src/CIPlatformUI" "CIPlatformUI"
WORKDIR /app/CIPlatformUI
RUN npm install
RUN npm run build --prod

FROM nginx:alpine AS final
EXPOSE 80
COPY --from=base /app/CIPlatformUI/dist/CIPlatformUI /usr/share/nginx/html