import axios from "@/http-common";
const files = "/files";

class homeService {
  getAllFiles() {
    return axios.get(`${files}`);
  }
  uploadFile(file) {
    axios.post(`${files}`, file);
  }
  getFileById(fileId) {
    return axios.get(`${files}/${fileId}`);
  }
  deleteFile(fileId) {
    return axios.delete(`${files}/${fileId}`);
  }
  downloadFile(fileId) {
    return axios.get(`${files}/getFileSource/${fileId}`);
  }
}

export default new homeService();
