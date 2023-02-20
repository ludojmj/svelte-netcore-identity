// api.js
import { get } from "svelte/store"
import axios from "axios";
import { apiErrMsg } from "./const.js";
import { isLoading, tokens } from "./store.js";

const rootApi = import.meta.env.VITE_API_URL;
const isMock = rootApi.indexOf("mock") > -1;
const axiosCallAsync = async (params) => {
  isLoading.set(true);
  try {
    const result = await axios(params);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  } finally {
    isLoading.set(false);
  }
};

export const apiSearchStuffAsync = async (search) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: `${rootApi}${mock}?search=${search}`
  };
  return axiosCallAsync(getMsg);
};

export const apiGetStuffListAsync = async () => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: rootApi + mock
  };
  return axiosCallAsync(getMsg);
};

export const apiGotoPageAsync = async (page) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: `${rootApi}${mock}?page=${page}`
  };
  return axiosCallAsync(getMsg);
};

export const apiGetStuffByIdAsync = async (id) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: `${rootApi}/${id}${mock}`
  };
  return axiosCallAsync(getMsg);
};

export const apiCreateStuffAsync = async (input) => {
  const mock = isMock ? ".json" : "";
  const postMsg = {
    method: isMock ? "get" : "post",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: rootApi + mock,
    data: input
  };
  return axiosCallAsync(postMsg);
};

export const apiUpdateStuffAsync = async (id, input) => {
  const mock = isMock ? ".json" : "";
  const putMsg = {
    method: isMock ? "get" : "put",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: `${rootApi}/${id}${mock}`,
    data: input
  };
  return axiosCallAsync(putMsg);
};

export const apiDeleteStuffAsync = async (id) => {
  const mock = isMock ? ".json" : "";
  const deleteMsg = {
    method: isMock ? "get" : "delete",
    headers: { Authorization: "Bearer " + get(tokens).accessToken },
    url: `${rootApi}/${id}${mock}`
  };
  return axiosCallAsync(deleteMsg);
};

const getErrorMsg = error => {
  const msg = apiErrMsg.generic;
  if (error.response && error.response.status === 401) {
    return apiErrMsg.unauthorized;
  }
  if (error.response && error.response.data && error.response.data.error) {
    return error.response.data.error;
  }
  if (error.message) {
    return error.message;
  }
  if (error) {
    return error;
  }
  return msg;
};
