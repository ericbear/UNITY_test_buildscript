### How to install and build

1. installation
```
curl -O "https://raw.githubusercontent.com/ericbear/UNITY_test_buildscript/master/package.json"
```

2. setup the configuration on `package.json`
> update the values of `configs`

3. build app through npm

  - android development build
  ```
  npm run build-prepare; npm run build-android
  ```
  
  - android production build
  ```
  npm run build-prepare; npm run build-android-production
  ```
  
  - ios development build
  ```
  npm run build-prepare; npm run build-ios
  ```
  
  - ios production build
  ```
  npm run build-prepare; npm run build-ios-production
  ```