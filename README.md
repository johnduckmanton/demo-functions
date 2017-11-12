# Demos & Samples of various Azure Functions
 
## Function Descriptions

### CaptionPhotos
This function takes a jpg image file stored in a defined container in Azure Blob Storage and passes it through the Azure Cognitive Services *Computer Vision* Analyze function. It then adds a caption & tags to the image as well as identifying any faces it finds, before writing the modified image out to a container in Azure Blob Storage.

To use this function you will need an Azure Subscription and a Computer Vision *Endpoint URI* and *API Key*. You can define these in the Application Settings for the function:

Setting | Description
------- | -----------
VISION_API_ENDPOINT | Computer Vision Endpoint URI 
VISION_API_KEY | Computer Vision API Key 
VISON_API_QUERYSTRING | QueryString containing the options required when calling the Computer Vision API (for example: *visualFeatures=Categories,Tags,Description,Faces,ImageType,Color&details=celebrities,landmarks&language=en)*

This demo is based on a function originally written by my colleague Ben Coleman (@benc-uk).
