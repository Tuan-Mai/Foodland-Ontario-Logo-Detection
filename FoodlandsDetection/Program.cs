﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;

/*
 * Detection for Foodlands Logo
 * 
 * Documentation was made and uploaded to Bitbucket
 */

namespace FoodlandsDetection
{
    public class FoodlandsTextDetection
    {

        const string usage = @"Usage:FoodlandsDetection <path_to_image>"; 

        /// <summary>
        /// Creates an authorized Cloud Vision client service using Application 
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Cloud Vision client.</returns>
        public VisionService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Vision scopes
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    VisionService.Scope.CloudPlatform
                });
            }
            return new VisionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }

        /// <summary>
        /// Detect text within an image using the Cloud Vision API.
        /// </summary>
        /// <param name="vision">an authorized Cloud Vision client.</param>
        /// <param name="imagePath">the path where the image is stored.</param>
        /// <returns>a list of text detected by the Vision API for the image.
        /// </returns>
        public IList<AnnotateImageResponse> DetectText(
            VisionService vision, string imagePath)
        {
            Console.WriteLine("Detecting Text...");
            // Convert image to Base64 encoded for JSON ASCII text based request   
            byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
            string imageContent = Convert.ToBase64String(imageArray);
            // Post text detection request to the Vision API
            var responses = vision.Images.Annotate(
                new BatchAnnotateImagesRequest()
                {
                    Requests = new[] {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type =
                          "TEXT_DETECTION"}},
                        Image = new Image() { Content = imageContent }
                    }
               }
                }).Execute();
            return responses.Responses;
        }

        private static void Main(string[] args)
        {
            FoodlandsTextDetection sample = new FoodlandsTextDetection();
            string imagePath;
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
                return;
            }
            imagePath = args[0];

            // Create a new Cloud Vision client authorized via Application 
            // Default Credentials
            VisionService vision = sample.CreateAuthorizedClient();
            // Use the client to get text annotations for the given image
            IList<AnnotateImageResponse> result = sample.DetectText(
                vision, imagePath);
            // Check for valid text annotations in response
            if (result[0].TextAnnotations != null)
            {
                // Loop through and output text annotations for the image
                foreach (var response in result)
                {
                    Console.WriteLine("Text found in image: " + imagePath);
                    Console.WriteLine();
                    foreach (var text in response.TextAnnotations)
                    {
                        Console.WriteLine(text.Description);
                        Console.Write("(Bounding Polygon: ");
                        var index = 0;
                        foreach (var point in text.BoundingPoly.Vertices)
                        {
                            if (index > 0) Console.Write(", ");
                            Console.Write("[" + point.X + "," + point.Y + "]");
                            index++;
                        }
                        Console.Write(")");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
            }
            else
            {
                if (result[0].Error == null)
                {
                    Console.WriteLine("No text found.");
                }
                else
                {
                    Console.WriteLine("Not a valid image.");
                }
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}