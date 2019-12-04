/**
 * Assign SXA jquery to $ namespace, to allow the usage of $ in CXA code base
 */
if (!window.$ && window.$xa) {
  window.$ = window.$xa;
}
