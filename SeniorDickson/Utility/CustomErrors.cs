using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Moirae.Utility
{
    public static class CustomErrors
    {
        public static string ShowError(int error)
        {
            string errorMessage = string.Empty;

            switch(error)
            {
                case 200:
                    errorMessage = "Thanks for testing out the error page. This is the page you will get when an error in the website occurs. Hopefully this is the last time you see this page.";
                    break;

                case 401:
                    errorMessage = "Oops! It seems you're trying to access a restricted area. We regret to inform you that access is denied. Double-check your credentials and make sure you're on the right path. Our friendly support team is here to assist you if you need any help.";
                    break;

                case 403:
                    errorMessage = "Hold on! You've reached a forbidden zone. We apologize for the inconvenience, but access to this area is strictly prohibited. It's like a secret treasure chest only accessible to a chosen few. Let's redirect you back to a safer spot.";
                    break;

                case 404:
                    errorMessage = "Uh-oh! The page you're looking for seems to have vanished into thin air. It's like searching for a needle in a haystack. We apologize for the confusion and invite you to explore other parts of our site. If you believe this is a mistake, please let us know.";
                    break;

                case 500:
                    errorMessage = "Oh no! Our servers are experiencing a temporary setback. It seems they've encountered a glitch in the matrix. Our team of tech experts is already on the case, working hard to restore order. Please bear with us as we come up a solution.";
                    break;

                case 502:
                    errorMessage = "Whoops-a-daisy! Our gateway to the outside world is facing a hiccup. It's like a bridge with a few missing planks. Our technical team is already on their way to fix this issue. In the meantime, we appreciate your patience.";
                    break;

                case 503:
                    errorMessage = "Hold your horses! Our services are taking a short breather at the moment. It appears they needed a moment to recharge their batteries. We apologize for any inconvenience caused and assure you that we'll be back up and running shortly.";
                    break;

                case 504:
                    errorMessage = "Oops! The gateway to our services seems to be taking a bit too long to respond. It's like waiting for a train that got stuck at a station. Our team is actively investigating the issue and working to restore the connection. We appreciate your understanding.";
                    break;
            }

            return errorMessage;
        }
    }
}
