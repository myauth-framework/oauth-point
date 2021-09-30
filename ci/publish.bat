echo off

IF [%1]==[] goto noparam

echo "Build image '%1' and 'latest'..."
docker build --progress plain -f ./Dockerfile -t ghcr.io/myauth-framework/oauth-point:%1 -t ghcr.io/myauth-framework/oauth-point:latest ../src

echo "Publish image '%1' ..."
docker push ghcr.io/myauth-framework/oauth-point:%1

echo "Publish image 'latest' ..."
docker push ghcr.io/myauth-framework/oauth-point:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"