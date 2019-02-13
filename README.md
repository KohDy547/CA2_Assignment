# Project Title

Cloud and Service Computing Assignment

## Description

Using ASP.Net Core 2.2, we develop the Life Time Talent website with Razor pages. Razor pages is a new aspect of ASP.NET Core MVC that makes coding page-focused scenarios easier and more productive as each page has its own model class which allow easier access of the model data.

### Prerequisites

Visual Studio installed with [.NET 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2)

### Installing

Clone the project onto a local machine

Add *appsetings.json* into your project directory

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=HOSTNAME;database=SCHEMA;uid=USERNAME;pwd=PASSWORD;",
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "App_SharedSettings": {
    "AwsCredentialsPath": "\\Data\\AwsCredentials",
    "RDS": "Server=HOSTNAME;database=SCHEMA;uid=USERNAME;pwd=PASSWORD;"
  },

  "Csc_StripeSettings": {
    "SecretKey": "[STRIPLE SECRET KEY HERE]",
    "PublishableKey": "[STRIPE PUBLISHABLE KEY HERE]"
  },
  "Csc_GoogleAuthSettings": {
    "ClientId": "[GOOGLE CLIENT ID HERE]",
    "ClientSecret": "[GOOGLE CLIENT SECRET HERE]"
  },
  "Csc_AwsS3Settings": {
    "Profile": "csc_credentials",
    "Region": "[S3 BUCKET REGION]",
    "Bucket": "[S3 BUCKET NAME HERE]",

    "Talents_FileKey": "Talents.json",
    "Talents_ImgBaseUrl": "https://s3-[S3 BUCKET REGION].amazonaws.com/[S3 BUCKET NAME HERE]/Talent_Photos/"
  },
  "AllowedHosts": "*"
}
```
Add *appsetings.Development.json* into your project directory

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

```
Add *AwsCredentials* into your project directory/data

```
[csc_credentials]
aws_access_key_id = [ACCESS KEY ID HERE]
aws_secret_access_key = [SECRET ACCESS KEY HERE]
```


## Running the tests

To test the program, run the IIS Express on your Visual Studio and check if you are able to
* Register
* Login
* Premium Member
* Upload Talents
* Delete Talents
* Post comments

## Publishing to Cloud
Using dontnet CLI, go to the directory of your application (with .csproj file)
```
dotnet publish  -c Release
```

## Built With

* [Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.2&tabs=visual-studio) - Model View Controller
* [Amazon EC2](https://aws.amazon.com/ec2/) - Host Instances
* [Amazon RDS](https://aws.amazon.com/rds/) - Database
* [Amazon S3](https://aws.amazon.com/s3/) - Store Talents
* [Stripe](https://stripe.com) - Used to make payments

## Authors

* **Koh Ding Yuan**
* **Jeffery Lau Wei Yang**
* **Nicholas Poh Yi Jie**
* **Ng Yu Xiang**
* **Bruce Wang**
