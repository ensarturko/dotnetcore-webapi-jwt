FROM microsoft/dotnet:2.0-sdk
WORKDIR /app/CoreApiUndockerized
COPY CoreApiUndockerized /app/CoreApiUndockerized
COPY CoreApiUndockerized.Data/ /app/CoreApiUndockerized.Data
 
RUN ["dotnet", "restore"] 
RUN ["dotnet", "build", "--configuration", "Release"]
 
ENV ASPNETCORE_URLS=http://*:1907
EXPOSE 1907
 
ENTRYPOINT ["dotnet", "run"]