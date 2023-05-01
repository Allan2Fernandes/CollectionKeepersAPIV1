FROM mcr.microsoft.com/mssql/server:2019-latest

# Set environment variables
ENV SA_PASSWORD=EasvEasv123! \
    ACCEPT_EULA=Y \
    MSSQL_PID=Express \
    MSSQL_TCP_PORT=1433

# Copy database creation script to the init directory
COPY create-database.sql /docker-entrypoint-initdb.d/

# Expose the SQL Server port
EXPOSE 1433
