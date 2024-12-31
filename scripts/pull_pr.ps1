param(
    [Parameter(mandatory=$true)]
    [string] $prNumber
)

# Requires: git remote add upstream https://github.com/naudio/NAudio/ 
git pull upstream pull/$prNumber/head;