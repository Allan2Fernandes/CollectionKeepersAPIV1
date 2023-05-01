FROM mcr.microsoft.com/mssql/server:2019-latest

# Set environment variables
ENV SA_PASSWORD EasvEasv123! 
ENV ACCEPT_EULA Y 
ENV MSSQL_PID Express 
ENV MSSQL_TCP_PORT 1433

# Expose the SQL Server port
EXPOSE 1433

# Copy database creation script to the init directory
COPY create-database.sql /docker-entrypoint-initdb.d
