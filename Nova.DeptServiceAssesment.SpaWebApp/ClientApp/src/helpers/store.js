import { createStore, applyMiddleware } from 'redux';
import thunkMiddleware from 'redux-thunk';
import promiseMiddleware from 'redux-promise';
import { createLogger } from 'redux-logger';
import rootReducer from '../reducers';

//const loggerMiddleware = createLogger();

//const middleWare = [promiseMiddleware, thunkMiddleware, loggerMiddleware];
const middleWare = [promiseMiddleware, thunkMiddleware]; //no log

export const store = applyMiddleware(...middleWare)(createStore)(rootReducer);