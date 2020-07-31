# PowerBI Embedded modules for DNN Platform
A set of modules to embed PowerBI Embedded dashboards and reports into a DNN Platform installation.

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

<a name="building"></a>
# Building the solution
### Requirements
* Visual Studio 2017 or later (download from https://www.visualstudio.com/downloads/)
* npm package manager (download from https://www.npmjs.com/get-npm)

### Configure local npm to use the DNN public repository
From the command line, the following command must be executed:
```
   npm config set registry https://www.myget.org/F/dnn-software-public/npm/
```
### Install package dependencies
From the command line, enter the `<RepoRoot>\DotNetNuke.PowerBI\PBIEmbedded.Web` and run the following commands:
```
  npm install -g webpack
  npm install -g webpack-cli
  npm install
```

### Build the module
Now you can build the solution by opening the file `DotNetNuke.PowerBI.sln` in Visual Studio. Building the solution in "Release", will generate the React bundle and package it all together with the installation zip file, created under the "\releases" folder.
