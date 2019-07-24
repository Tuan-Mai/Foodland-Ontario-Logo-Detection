# Foodland-Ontario-Logo-Detection

The Foodland Ontario Logo Detection project utilizes machine learning and computer vision to search for Foodland Ontario Logos on provided images.

This detection uses Google's Vision AI to learn from a provided sample image (only requires one image) for the AI to learn and understand the objects of interest.

This project can work as a simple console command project which will tell the users how many logos were detected, as well as provide coordinates (x, y, width, length) of the bounding box of each found logo. 

To accurately determine if the logos are correctly identified, an extra layer of optical character recognition (OCR) is used to check for logos. 
