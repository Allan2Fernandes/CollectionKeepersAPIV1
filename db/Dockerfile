FROM mcr.microsoft.com/mssql/server:2019-latest

# Copy initialization scripts
COPY /initScripts .

# Set environment variables
ENV SA_PASSWORD EasvEasv123! 
ENV ACCEPT_EULA Y 
ENV MSSQL_PID Express 
ENV MSSQL_TCP_PORT 1433

# Expose the SQL Server port
EXPOSE 1433

# Run Microsoft SQL Server and initialization script (at the same time)
CMD /bin/bash ./entrypoint.sh
