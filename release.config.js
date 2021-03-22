{
  "plugins": [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    "@semantic-release/changelog",
    "@semantic-release/git",
    ["@semantic-release/exec", {
      "prepareCmd": "dotnet pack -v normal -c Release --include-symbols --include-source -p:PackageVersion=${nextRelease.version} -o nupkg ${process.env.PROJECT_NAME}/${process.env.PROJECT_NAME}.*proj",
      "publishCmd": "dotnet nuget push nupkg/*.nupkg --source ${process.env.NUGET_FEED} --skip-duplicate --api-key ${process.env.NUGET_TOKEN}"
    }],
      ["@semantic-release/github", {
      "assets": [
        {"path": "nuget/*.nupkg"}
      ]
    }],
  ]
}