import axios from 'axios';

// const apiUrl = "https://localhost:5047"
axios.defaults.baseURL = "https://localhost:7271";

axios.interceptors.response.use(
  response => response, // אם אין שגיאה, מחזירים את התגובה
  error => {
    // תופסים את השגיאה ומדפיסים ללוג
    console.error('API ERROR:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/items`)
    return result.data;
  },

  addTask: async (name) => {
    console.log('addTask', name)
    const result = await axios.post(`/items`, { name })
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete })
    const result = await axios.put(`/items/${id}`, { isComplete })
    return result.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask')
    const result = await axios.delete(`/items/${id}`)
    return result.data;
  }
};
