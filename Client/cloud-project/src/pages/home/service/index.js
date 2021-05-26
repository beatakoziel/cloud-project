import axios from "@/http-common";
const files = "/files";
const directories = "/directories";

class homeService {
  getAllFiles(dirId) {
    return axios.get(`${files}/getFiles/${dirId}`);
  }
  uploadFile(formData, dirId) {
    return axios.post(`${files}/addFile/${dirId}`, formData);
  }
  getFileById(fileId) {
    return axios.get(`${files}/${fileId}`);
  }
  deleteFile(fileId) {
    return axios.delete(`${files}/${fileId}`);
  }
  downloadFile(fileId) {
    return axios.get(`${files}/${fileId}/source`, {
      responseType: "arraybuffer",
    });
  }
  getDirectories(dirId) {
    return axios.get(`${directories}/getDirectories/${dirId}`);
  }
  addDirectory(dirId, dirName) {
    return axios.post(`${directories}/${dirId}/${dirName}`);
  }
  getCurrentDirName(dirId) {
    return axios.get(`${directories}/getCurrentDirName/${dirId}`)
  }
}

export default new homeService();
