import { makeUrl } from '../router/makeUrl';

export const URL_HOME = new makeUrl('/');
export const URL_LOGIN = new makeUrl('/login');
export const URL_SIGN_UP = new makeUrl('/sing-up');
export const URL_VERIFY = new makeUrl('/verify');
export const URL_VERIFY_INFO = new makeUrl('/verify-info');
export const URL_FORBIDDEN = new makeUrl('/403');
export const URL_NOT_FOUND = new makeUrl('/404');
export const URL_QUIZ_INTRO = new makeUrl('/quiz/:id/intro');
export const URL_QUIZ = new makeUrl('/quiz/:id');
