# Local Development

```
POST http://localhost:7071/api/SaveImage
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW

------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="ImageData"

{ "someKey": "someValue" }
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Image"; filename="SampleImage.png"
Content-Type: image/png

< ./SampleImage.png
------WebKitFormBoundary7MA4YWxkTrZu0gW--
```

# Production

```
POST https://vsdraw.azurewebsites.net/api/SaveImage
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW

------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="ImageData"

{ "someKey": "someValue" }
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Image"; filename="SampleImage.png"
Content-Type: image/png

< ./SampleImage.png
------WebKitFormBoundary7MA4YWxkTrZu0gW--
```
