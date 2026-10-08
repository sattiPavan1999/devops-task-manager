# Stage 1: Build React Assets
FROM node:18-alpine AS build
WORKDIR /app

# Copy dependency definitions and install
COPY package*.json ./
RUN npm install

# Allow overriding the API URL at build time; default to same-origin requests for Kubernetes.
ARG VITE_API_BASE_URL=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

# Copy source and compile production bundle
COPY . .
RUN npm run build

# Stage 2: Serve with Nginx Alpine
FROM nginx:alpine AS final

# Copy build artifacts to Nginx HTML root
COPY --from=build /app/dist /usr/share/nginx/html

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]