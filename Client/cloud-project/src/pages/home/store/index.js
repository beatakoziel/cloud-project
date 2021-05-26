import { getField, updateField } from "vuex-map-fields";
import service from "../service";
import router from '@/router'

const namespaced = true;

const state = {
  filesList: [],
  file: {
    name: "",
    type: "",
    source: "",
  },
  directory: {
    dirName: "",
  },
  previousVersions: [],
  directoriesList: [],
  currentDirName: "",
};

const getters = {
  getField,
  getFilesList: (state) => state.filesList,
  getDirectoriesList: (state) => state.directoriesList,
  getCurrentDirName: (state) => state.currentDirName,
};

const mutations = {
  updateField,
  SET_FILES_LIST: (state, payload) => {
    state.filesList = payload;
  },
  SET_DIRECTORIES_LIST: (state, payload) => {
    state.directoriesList = payload;
  },
  SET_CURRENT_DIR_NAME: (state, payload) => {
    state.currentDirName = payload;
  }
};

const actions = {
  setFilesList: async ({ rootState, commit }) => {
    await service.getAllFiles(router.currentRoute.params.dirId).then((response) => {
      commit("SET_FILES_LIST", response.data);
    });
  },
  uploadFile: async ({ rootState }, formData) => {
    await service.uploadFile(formData, router.currentRoute.params.dirId).catch((error) => {
      console.log(error);
    });
  },
  downloadFile: ({ commit }, fileId) => {
    return new Promise((resolve, reject) => {
      service
        .downloadFile(fileId)
        .then((response) => {
          resolve(response);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },
  deleteFile: async ({ commit }, fileId) => {
    await service.deleteFile(fileId).catch((error) => {
      console.log(error);
    });
  },
  addDir: async ({ rootState }, dirName) => {
    await service.addDirectory(router.currentRoute.params.dirId, dirName).catch((error) => {
      console.log(error);
    });
  },
  setDirectoriesList: async ({ rootState, commit }) => {
    await service.getDirectories(router.currentRoute.params.dirId).then((response) => {
      commit("SET_DIRECTORIES_LIST", response.data);
    });
  },
  setCurrentDirName: ({ commit }) => {
    service.getCurrentDirName(router.currentRoute.params.dirId).then((response) => {
      commit("SET_CURRENT_DIR_NAME", response.data);
    })
  }
};

export default { state, getters, mutations, actions, namespaced };
