import { configConstants as config } from '../constants';
import { authenService } from './authProvider';
import axios, { post } from 'axios';



export const DataService = {
   
    getUserPeriods : async (activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/userassessments/periods?activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    getUserAssessmentDepartments : async (period) => {
    
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/userassessments/periods/${period}`, opt);
    },

    getUserAssessmentQuestions : async (period, department) => {
    
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/userassessments/periods/${period}/departments/${department}`, opt);
    },

    saveAssessmentResult : async (postData, isComplete) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: isComplete ? '/api/userassessments/results/complete' : '/api/userassessments/results',
            headers: _headers,
            data: JSON.stringify(postData)
        }

        return axios(opt);
    },

    getAdminQuestions : async (activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/questions?activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    getQuestionCriteria : async () => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/criteria`, opt);

    },

    getQuestion : async (id) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/questions/${id}`, opt);

    },

    saveQuestion : async (postData) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: '/api/adminassessments/questions',
            headers: _headers,
            data: JSON.stringify(postData)
        }

        return axios(opt);

    },

    getDepartmentMatrix : async () => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/departments/matrix`, opt);

    },

    updateDepartmentMatrix : async (file) => {
        
        const authenTotken = await await  authenService.getToken();
        const url = '/api/adminassessments/departments/matrix/import';
        const formData = new FormData();
        formData.append('file',file)
        const config = {
            headers: {
                'content-type': 'multipart/form-data',
                'Authorization': 'Bearer ' + authenTotken
            }
        }
        return  post(url, formData, config);

    },

    getAdminPeriods : async (activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/periods?activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    getAdminPeriod : async (id) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/periods/${id}`, opt);

    },

    getAdminPeriodDepartments : async (id, activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/periods/${id}/departments?activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    getAdminPeriodUsers : async (id) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/periods/${id}/users`, opt);

    },

    saveAdminPeriod : async (postData) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: '/api/adminassessments/periods',
            headers: _headers,
            data: JSON.stringify(postData)
        }

        return axios(opt);

    },

    notifyPeriod : async (postData) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: `/api/adminassessments/periods/${postData.id}/notify`,
            headers: _headers,
            data: JSON.stringify(postData)
        }

        return axios(opt);

    },

    remindPeriod : async (postData) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: `/api/adminassessments/periods/${postData.id}/remind`,
            headers: _headers,
            data: JSON.stringify(postData)
        }

        return axios(opt);

    },

    getReport : async (type, periodId, departmentId) => {
        
        // console.log(type);
        // console.log(periodId);

        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }
        
        return axios(`/api/adminassessments/reports/${type}/periods/${periodId}/departments/${departmentId}`, opt);

    },

    getDepartments : async (searchterm, activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        searchterm = encodeURI(searchterm);

        return axios(`/api/adminassessments/departments?q=${searchterm}&activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    syncDepartments : async () => {
        
        let authenTotken = await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: '/api/adminassessments/departments/sync',
            headers: _headers
        }

        return axios(opt);

    },

    importDepartments : async (file) => {
        
        const authenTotken = await await  authenService.getToken();
        const url = '/api/adminassessments/departments/import';
        const formData = new FormData();
        formData.append('file',file)
        const config = {
            headers: {
                'content-type': 'multipart/form-data',
                'Authorization': 'Bearer ' + authenTotken
            }
        }
        return  post(url, formData, config);

    },

    getUsers : async (searchterm, activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        searchterm = encodeURI(searchterm);

        return axios(`/api/adminassessments/users?q=${searchterm}&activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    syncUsers : async () => {
        
        let authenTotken = await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        let opt = {
            method: 'POST',
            url: '/api/adminassessments/users/sync',
            headers: _headers
        }

        return axios(opt);

    },
 
    importUsers : async (file) => {
        
        const authenTotken = await await  authenService.getToken();
        const url = '/api/adminassessments/users/import';
        const formData = new FormData();
        formData.append('file',file)
        const config = {
            headers: {
                'content-type': 'multipart/form-data',
                'Authorization': 'Bearer ' + authenTotken
            }
        }
        return  post(url, formData, config);

    },

    getPermissions : async (activePage, itemsPerPage) => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/permissions?activePage=${activePage}&itemsPerPage=${itemsPerPage}`, opt);

    },

    savePermission : async (email) => {
        
        let authenTotken = await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        const dataPost = {
            email: email
        }
        let opt = {
            method: 'POST',
            url: '/api/adminassessments/permissions',
            headers: _headers,
            data: JSON.stringify(dataPost)
        }

        return axios(opt);

    },

    deletePermission : async (email) => {
        
        let authenTotken = await await  authenService.getToken();
        let _headers = {
            "Content-Type" : "application/json",
            'Authorization': 'Bearer ' + authenTotken
        }

        const dataPost = {
            email: email
        }
        let opt = {
            method: 'DELETE',
            url: '/api/adminassessments/permissions',
            headers: _headers,
            data: JSON.stringify(dataPost)
        }

        return axios(opt);

    },

    checkPermission : async () => {
        
        let opt = {
            method: 'GET',
            headers: await authenService.authHeader()
        }

        return axios(`/api/adminassessments/permissions/check`, opt);

    },
}

