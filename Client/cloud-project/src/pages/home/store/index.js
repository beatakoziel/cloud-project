import { getField, updateField } from "vuex-map-fields";
import service from "../service";

const namespaced = true;

const state = {
  filesList: [],
  file: {
    name: "",
    type: "",
    source: "",
  },
};

const getters = {
  getField,
  getFilesList: (state) => state.filesList,
};

const mutations = {
  updateField,
  SET_FILES_LIST: (state, payload) => {
    state.filesList = payload;
  },
};

const actions = {
  setFilesList: ({ commit }) => {
    service.getAllFiles().then((response) => {
      commit("SET_FILES_LIST", response.data);
    });
  },
  uploadFile: ({ state }) => {
    service.uploadFile(state.file);
  },
  downloadFile: ({ commit }, fileId) => {
    return new Promise((resolve, reject) => {
      service
        .downloadFile(fileId)
        .then((response) => {
          resolve(response.data);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },
  deleteFile: ({ commit }, fileId) => {
    service.deleteFile(fileId);
  },
};

export default { state, getters, mutations, actions, namespaced };
