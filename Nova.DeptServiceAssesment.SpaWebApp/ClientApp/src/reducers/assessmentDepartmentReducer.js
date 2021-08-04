const initialState = {
  departments: null,
};
 
export function assessmentDepartmentReducer(state = initialState, action) {
  switch (action.type) {
    case 'CHANGE_DEPARTMENT_LIST':
      return { ...state, departments: action.payload };
    default:
      return state;
  }
}