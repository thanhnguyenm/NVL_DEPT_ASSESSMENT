const initialState = {
    isOpenModal: false,
    title: '',
    content: ''
};
   
export function modalReducer(state = initialState, action) {
    switch (action.type) {
        case 'MODAL_TOGGLE':
            return { ...state, isOpenModal: action.payload.isOpenModal, title: action.payload.title, content: action.payload.content };
        default: 
            return state;
    }
}