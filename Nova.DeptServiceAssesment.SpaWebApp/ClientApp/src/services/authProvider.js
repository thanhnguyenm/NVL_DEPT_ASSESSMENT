import { azureAdConfig } from '../constants'
import { MsalAuthProvider, LoginType } from 'react-aad-msal';

// Msal Configurations
const config = {
    auth: {
      authority: `${azureAdConfig.instance}${azureAdConfig.tenant}/`,
      clientId: azureAdConfig.clientId,
      redirectUri: azureAdConfig.redirectUri
    },
    cache: {
      cacheLocation: "sessionStorage",
      storeAuthStateInCookie: true
    }
  };
   
  // Authentication Parameters
  const authenticationParameters = {
    scopes: [
      "openid", "profile", 'user.read'
    ]
  }
   
  // Options
  const options = {
    loginType: LoginType.Redirect,
    tokenRefreshUri: window.location.origin + '/auth.html'
  }
   
  export const authProvider = new MsalAuthProvider(config, authenticationParameters, options)

  const getToken = async () => {
    const token = await authProvider.getIdToken();
    const idToken = token.idToken.rawIdToken; 

    return idToken;
  }

  const authHeader = async () => {
    var idtoken = await getToken();
    return { 'Authorization': 'Bearer ' + idtoken };
  }
  export const authenService = {
    getToken,
    authHeader,
  }