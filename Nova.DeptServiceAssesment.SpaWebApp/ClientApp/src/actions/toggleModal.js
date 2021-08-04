
export const toggleModal = (isOpenModal, title, content)=>{
    return (dispatch, getState)=>{
        //call this function in component
        dispatch({
            type: "MODAL_TOGGLE",
            payload: {
                isOpenModal,
                title,
                content
            }
        })
    }
}
