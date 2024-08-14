using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using CafeteriaClient.DTO.Response;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Employee
{
    public class UpdateProfileCommand : ICommand
    {
        private readonly Func<int> _getUserId;

        public UpdateProfileCommand(Func<int> getUserId)
        {
            _getUserId = getUserId;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            var userId = _getUserId();

            // Fetch existing preferences
            var fetchRequest = new RequestObject
            {
                CommandName = "getEmployeePreference",
                RequestData = JsonConvert.SerializeObject(new EmployeePreferenceRequest { UserId = userId })
            };

            string preferenceResponseJson = await clientSocket.SendRequest(fetchRequest);
            var preferenceResponse = JsonConvert.DeserializeObject<EmployeePreferenceResponse>(preferenceResponseJson);
            bool validChoice = false;

            if (preferenceResponse.IsSuccess && preferenceResponse.PreferenceResponse != null)
            {
                DisplayExistingPreferences(preferenceResponse.PreferenceResponse);

                Console.WriteLine("Options:");
                Console.WriteLine("1) Update Profile");
                Console.WriteLine("2) Exit");

                while (!validChoice)
                {
                    var choice = Console.ReadLine();
                    if (choice == "1")
                    {
                        validChoice = true;
                    }
                    else if (choice == "2")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Please enter 1 to update profile or 2 to exit.");
                    }
                }
            }

            if(validChoice)
            {
                Console.WriteLine("Please answer these questions to know your preferences");

                int foodPreference = GetValidInput("1) Please select your food preference:", new string[] { "Vegetarian", "Non Vegetarian", "Eggetarian" });
                int spiceLevel = GetValidInput("2) Please select your spice level:", new string[] { "High", "Medium", "Low" });
                int cuisinePreference = GetValidInput("3) What do you prefer most?", new string[] { "North Indian", "South Indian", "Other" });
                bool hasSweetTooth = GetYesOrNoInput("4) Do you have a sweet tooth?");

                var request = new UpdateProfileRequest
                {
                    UserId = userId,
                    FoodTypeId = foodPreference,
                    SpiceLevelId = spiceLevel,
                    CuisineTypeId = cuisinePreference,
                    HasSweetTooth = hasSweetTooth
                };

                var requestObject = new RequestObject
                {
                    CommandName = "updateProfile",
                    RequestData = JsonConvert.SerializeObject(request)
                };

                string responseJson = await clientSocket.SendRequest(requestObject);
                var response = JsonConvert.DeserializeObject<UpdateProfileResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Profile updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to update profile: {response.ErrorMessage}");
                }
            }
        }

        private void DisplayExistingPreferences(PreferenceResponse preference)
        {
            Console.WriteLine("\nProfile:\nYour Preferences:");
            Console.WriteLine($"Food Preference: {preference.FoodPreference}");
            Console.WriteLine($"Spice Level: {preference.SpiceLevel}");
            Console.WriteLine($"Cuisine Preference: {preference.CuisinePreference}");
            Console.WriteLine($"Has Sweet Tooth: {(preference.HasSweetTooth ? "Yes" : "No")}");
            Console.WriteLine();
        }

        private int GetValidInput(string prompt, string[] options)
        {
            int choice = -1;
            while (choice < 1 || choice > options.Length)
            {
                Console.WriteLine(prompt);
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {options[i]}");
                }

                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > options.Length)
                {
                    Console.WriteLine("Invalid input. Please enter a number corresponding to the options.");
                }
            }
            return choice;
        }

        private bool GetYesOrNoInput(string prompt)
        {
            int choice = -1;
            while (choice < 1 || choice > 2)
            {
                Console.WriteLine(prompt);
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");

                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 2)
                {
                    Console.WriteLine("Invalid input. Please enter 1 for Yes or 2 for No.");
                }
            }
            return choice == 1;
        }
    }
}
