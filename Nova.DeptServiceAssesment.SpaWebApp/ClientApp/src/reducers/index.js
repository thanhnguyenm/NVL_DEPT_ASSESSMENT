import { combineReducers } from 'redux';

import { authenticationReducer } from './authenticationReducer';
import { assessmentDepartmentReducer } from './assessmentDepartmentReducer';
import { modalReducer } from './modalReducer';

const rootReducer = combineReducers({
  authenticationData : authenticationReducer,
  assessmentDepartmentData : assessmentDepartmentReducer,
  modalData : modalReducer
});

export default rootReducer;