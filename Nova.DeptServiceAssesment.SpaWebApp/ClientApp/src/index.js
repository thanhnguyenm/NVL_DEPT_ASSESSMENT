import 'react-app-polyfill/ie9'; // For IE 9-11 support
import 'react-app-polyfill/stable';
import './polyfill'
import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';
//https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-js-initializing-client-applications#configuration-options
//https://www.npmjs.com/package/react-aad-msal
import { AzureAD } from 'react-aad-msal'; 
import { authProvider } from './services';
import { Provider } from 'react-redux'

import { store } from './helpers';

ReactDOM.render(
    <AzureAD provider={authProvider} forceLogin={true} reduxStore={store}>
        <Provider  store={store}>
            <App />
        </Provider>
    </AzureAD>,
    document.getElementById('root'),
);


// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: http://bit.ly/CRA-PWA
serviceWorker.unregister();
