using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthenticationState AuthState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> Authenticate(int maxTries = 5)
    {
        if(AuthState == AuthenticationState.Authenticated)
        {
            return AuthState;
        }

        if(AuthState ==  AuthenticationState.Authenticating)
        {
            Debug.Log("Already authenticating");
            await CheckAuthenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);

        return AuthState;
    }

    private static async Task<AuthenticationState> CheckAuthenticating()
    {
        while(AuthState == AuthenticationState.Authenticating ||  AuthState == AuthenticationState.NotAuthenticated)
        {
            int DelayBetweenAuthenticating = 300;
            await Task.Delay(DelayBetweenAuthenticating);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthenticationState.Authenticating;
        int tries = 0;

        while (AuthState == AuthenticationState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException ex)
            {
                Debug.LogError(ex.Message);
                AuthState = AuthenticationState.Error;
            }
            catch(RequestFailedException ex)
            {
                Debug.LogError(ex.Message);
                AuthState= AuthenticationState.Error;
            }

            tries++;
            int waitBeforeTryingAgain = 1000; //Milliseconds
            await Task.Delay(waitBeforeTryingAgain);
        }

        if(AuthState != AuthenticationState.Authenticated)
        {
            Debug.LogWarning("Player was unable to sign in");
            AuthState = AuthenticationState.TimeOut;
        }
    }
}
