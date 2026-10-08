FROM postgres:16-alpine

# Environment variables setting default DB credentials
ENV POSTGRES_USER=taskuser
ENV POSTGRES_PASSWORD=taskpassword
ENV POSTGRES_DB=taskmanagerdb

# Copy initialization scripts into the auto-executing entrypoint folder
COPY init-scripts/ /docker-entrypoint-initdb.d/

EXPOSE 5432