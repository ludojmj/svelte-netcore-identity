// lib/store.js
import { writable } from "svelte/store";

export const isAuthLoading = writable(false);
isAuthLoading.subscribe(value => value);

export const isLoading = writable(false);
isLoading.subscribe(value => value);

export const selectedItem = writable(JSON.parse(localStorage.getItem("selectedItem")) || {});
selectedItem.subscribe(val => localStorage.setItem("selectedItem", JSON.stringify(val)));

export const tokens = writable(null);
tokens.subscribe(value => value);

export const userInfo = writable(null);
userInfo.subscribe(value => value);