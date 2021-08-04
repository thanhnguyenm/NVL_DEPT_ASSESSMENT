
export const changeDepartmentList = (departments)=>{
    return (dispatch, getState)=>{
        //call this function in component
        dispatch({
            type: "CHANGE_DEPARTMENT_LIST",
            payload: departments
        })
    }
}
