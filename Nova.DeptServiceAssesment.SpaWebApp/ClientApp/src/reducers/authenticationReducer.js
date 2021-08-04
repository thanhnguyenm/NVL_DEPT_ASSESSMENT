// import { userConstants } from '../constants';
// import { getProfile, getToken } from '../services';

const initialState = {
  aadResponse: null
};
 
export function authenticationReducer(state = initialState, action) {
  switch (action.type) {
    case 'AAD_LOGIN_SUCCESS':
      return { ...state, aadResponse: action.payload };
    case 'AAD_LOGOUT_SUCCESS':
      return { ...state, aadResponse: null };
    default:
      return state;
  }
}