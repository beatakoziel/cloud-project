import axios from "axios";

export default axios.create({
  baseURL: "http://localhost:53033",
  headers: {
    "Content-type": "application/json",
  },
});
