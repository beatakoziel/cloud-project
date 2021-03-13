import { getField, updateField } from "vuex-map-fields";
import service from "../service";

const namespaced = true;

const state = {
  filesList: [],
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
};

export default { state, getters, mutations, actions, namespaced };
