# Infinum Academy | DevOps

## Example .NET application for Infinum DevOps Academy - Movies DB

### Database

By default, the application will use InMemory database. If you set the environment variable (`ConnectionStrings__MoviesDb`), then app will use PostgreSQL:

```
ConnectionStrings__MoviesDb: "Host=localhost;Username=postgres;Database=movies_development"
```

### BLOB Storage Serivce

By default, the application will use **local disk storage**. If you set the environment variable (`AWS__S3__*`), then app will use AWS S3:

```
AWS__S3__AccessKeyID: "000000"
AWS__S3__SecretAccessKey: "000000"
AWS__S3__Region: "us-east-1"
AWS__S3__ServiceURL: "http://localhost:4566"
```


### Environment Variables

```
ConnectionStrings__MoviesDb: "Host=localhost;Username=postgres;Database=movies_development"
AWS__S3__AccessKeyID: "000000"
AWS__S3__SecretAccessKey: "000000"
AWS__S3__Region: "us-east-1"
AWS__S3__ServiceURL: "http://localhost:4566"
```
