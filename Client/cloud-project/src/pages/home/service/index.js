import axios from "@/http-common";
const files = "/file";

class homeService {
  getAllFiles() {
    return axios.get(`${files}/getAllFiles`);
  }
}

export default new homeService();
