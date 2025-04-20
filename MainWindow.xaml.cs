// using System.Text;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Data;
// using System.Windows.Documents;
// using System.Windows.Input;
// using System.Windows.Media;
// using System.Windows.Media.Imaging;
// using System.Windows.Navigation;
// using System.Windows.Shapes;

// namespace MyWpfApp;

// /// <summary>
// /// Interaction logic for MainWindow.xaml
// /// </summary>
// public partial class MainWindow : Window
// {
//     public MainWindow()
//     {
//         InitializeComponent();
//     }
// }
using System;
using System.Net.Http; // Required for HttpClient
using System.Net.Http.Json; // Required for ReadFromJsonAsync (install package if needed)
using System.Text;
using System.Text.Json; // Required for JSON handling
using System.Threading.Tasks; // Required for async/await
using System.Windows;

namespace OllamaChatApp
{
    public partial class MainWindow : Window
    {
        // Reuse HttpClient for better performance
        private static readonly HttpClient client = new HttpClient();
        private const string OllamaApiUrl = "http://localhost:11434/api/generate"; // Default Ollama API endpoint
        private const string DefaultModel = "gemma3:1b"; // Change this if you use a different default model

        public MainWindow()
        {
            InitializeComponent();
            // Set a default timeout for the HttpClient
            client.Timeout = TimeSpan.FromMinutes(5); // Adjust as needed for long responses
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userPrompt = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                MessageBox.Show("Please enter a prompt.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- UI Updates: Indicate Busy ---
            SendButton.IsEnabled = false;
            ResponseTextBlock.Text = string.Empty; // Clear previous response
            StatusTextBlock.Text = "Sending request to Ollama...";
            // ---

            try
            {
                // --- Prepare the request ---
                var requestData = new OllamaRequest
                {
                    Model = DefaultModel, // Use the model you have downloaded
                    Prompt = userPrompt,
                    Stream = false // Set to false for a single complete response
                                   // Set to true for streaming (requires different handling)
                };

                // --- Send the request ---
                // Using PostAsJsonAsync simplifies sending JSON
                HttpResponseMessage response = await client.PostAsJsonAsync(OllamaApiUrl, requestData);

                // --- Process the response ---
                if (response.IsSuccessStatusCode)
                {
                    // For non-streaming response (stream = false)
                    OllamaResponse ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaResponse>();
                    if (ollamaResponse != null)
                    {
                         // Update UI on the UI thread (though await often handles this in WPF)
                        Dispatcher.Invoke(() =>
                        {
                            ResponseTextBlock.Text = ollamaResponse.Response ?? "No response content received.";
                            StatusTextBlock.Text = $"Response received ({ollamaResponse.TotalDuration / 1e9:F2}s). Ready.";
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() => StatusTextBlock.Text = "Error: Could not parse Ollama response.");
                        MessageBox.Show("Received an empty or invalid response from Ollama.", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Read the error response body if available
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Dispatcher.Invoke(() => StatusTextBlock.Text = $"Error: {response.ReasonPhrase}");
                    MessageBox.Show($"Failed to get response from Ollama.\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}\nDetails: {errorBody}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle network-related errors (e.g., Ollama not running)
                Dispatcher.Invoke(() => StatusTextBlock.Text = "Error: Connection failed.");
                MessageBox.Show($"Could not connect to Ollama API at {OllamaApiUrl}.\nPlease ensure Ollama is running.\n\nError: {httpEx.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (JsonException jsonEx)
            {
                // Handle errors during JSON deserialization
                Dispatcher.Invoke(() => StatusTextBlock.Text = "Error: Invalid response format.");
                MessageBox.Show($"Error parsing JSON response from Ollama.\n\nError: {jsonEx.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (TaskCanceledException taskEx)
             {
                 // Handle timeouts
                 Dispatcher.Invoke(() => StatusTextBlock.Text = "Error: Request timed out.");
                 MessageBox.Show($"The request to Ollama timed out.\n\nError: {taskEx.Message}", "Timeout Error", MessageBoxButton.OK, MessageBoxImage.Warning);
             }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                 Dispatcher.Invoke(() => StatusTextBlock.Text = "An unexpected error occurred.");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // --- UI Updates: Indicate Ready ---
                // Ensure UI updates happen on the UI thread
                 Dispatcher.Invoke(() => SendButton.IsEnabled = true);
                 // Keep the status text unless it was just updated with the result time
                 if (!StatusTextBlock.Text.Contains("Response received")) {
                    Dispatcher.Invoke(() => StatusTextBlock.Text = "Ready");
                 }
                // ---
            }
        }

        // --- Helper Classes for JSON Deserialization ---

        // Represents the request payload sent to Ollama
        private class OllamaRequest
        {
            [System.Text.Json.Serialization.JsonPropertyName("model")]
            public string Model { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("prompt")]
            public string Prompt { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("stream")]
            public bool Stream { get; set; }
        }

        // Represents the response structure from Ollama (when stream = false)
        // Note: Ollama's full response has more fields, this captures the essential part
        private class OllamaResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("model")]
            public string Model { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("response")]
            public string Response { get; set; } // The actual generated text

            [System.Text.Json.Serialization.JsonPropertyName("done")]
            public bool Done { get; set; }

            // You might want to add other fields like timings if needed
            [System.Text.Json.Serialization.JsonPropertyName("total_duration")]
            public long TotalDuration { get; set; } // Duration in nanoseconds

             // Add other fields as needed from the Ollama API documentation:
             // https://github.com/ollama/ollama/blob/main/docs/api.md#generate-a-completion
        }
    }
}