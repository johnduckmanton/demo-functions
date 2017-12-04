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

### ProcessCSVFile
This function takes as input a csv file containing the details of Titanic survivors (see [TitanicTab.csv](ProcessCSVFile/TitanicTab.csv) for the sample file) and does some processing on it:

* The passenger name column (pas_name) is expected to be in the format "surname, title forenames"
This function will to reformat this to "title forenames surname" and store the result in a new column (pas_name2).

* If any errors are found in the pas_name format a '1' will be placed in a new column 
called "name_format_error" on corresponding row.