// stuffList.js
import axios from "axios";

const rootApi = isProd ? "https://localhost:5001/api/stuff" : "http://localhost:3000/mock/stuff";
const isMock = rootApi.indexOf("mock") > -1;
const axiosCall = async (params) => {
  try {
    const result = await axios(params);
    return result.data;
  } catch (error) {
    const errResult = { error: getErrorMsg(error) };
    return errResult;
  }
};

export const apiGetStuffList = async (accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: rootApi + mock
  };
  return await axiosCall(getMsg);
};

export const apiSearchStuff = async (search, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}${mock}?search=${search}`
  };
  return await axiosCall(getMsg);
};

export const apiGotoPage = async (page, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}${mock}?page=${page}`
  };
  return await axiosCall(getMsg);
};

export const apiGetStuffById = async (id, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`
  };
  return await axiosCall(getMsg);
};

export const apiCreateStuff = async (input, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const postMsg = {
    method: isMock ? "get" : "post",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: rootApi + mock,
    data: input
  };
  return await axiosCall(postMsg);
};

export const apiUpdateStuff = async (id, input, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const putMsg = {
    method: isMock ? "get" : "put",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`,
    data: input
  };
  return await axiosCall(putMsg);
};

export const apiDeleteStuff = async (id, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const deleteMsg = {
    method: isMock ? "get" : "delete",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`
  };
  return await axiosCall(deleteMsg);
};

const getErrorMsg = error => {
  const msg = "An error occured. Try again later.";
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