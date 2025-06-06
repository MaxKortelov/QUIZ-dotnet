import React, { lazy } from 'react';
import { createBrowserRouter, Navigate } from 'react-router-dom';
import { Layout } from 'components/Layout';

import { PublicRoute } from 'components/Routes/PublicRoute';
import { PrivateRoute } from 'components/Routes/PrivateRoute';
import { SideMenuItemsEnum } from "components/Layout/SideMenu/items";

import {
  URL_HOME,
  URL_FORBIDDEN,
  URL_NOT_FOUND,
  URL_LOGIN,
  URL_SIGN_UP,
  URL_VERIFY,
  URL_VERIFY_INFO,
  URL_QUIZ_INTRO,
  URL_QUIZ,
} from '../constants/clientUrl';

import {
  SignInForm,
  SignUpForm,
  VerifyAccount,
  VerifyAccountInfo,
  Authorization
} from "features/Authorization";

const NotFound = lazy(() => import('features/ErrorPages/NotFound'));
const Forbidden = lazy(() => import('features/ErrorPages/Forbidden'));
const Home = lazy(() => import('features/Home'));
const QuizIntro = lazy(() => import('features/QuizIntro'));
const Quiz = lazy(() => import('features/Quiz'));

export const router = createBrowserRouter([
  {
    element: <PrivateRoute />,
    children: [
      {
        path: '*',
        element: <Navigate to={URL_NOT_FOUND.route} />,
      },
      {
        element: <Layout />,
        children: [
          {
            path: URL_NOT_FOUND.route,
            element: (
                <NotFound />
            ),
          },
          {
            path: URL_FORBIDDEN.route,
            element: (
                <Forbidden />
            ),
          },
          {
            path: URL_QUIZ_INTRO.route,
            element: (
                <QuizIntro />
            ),
          },
          {
            path: URL_QUIZ.route,
            element: (
                <Quiz />
            ),
          },
        ],
      },
      {
        element: <Layout activeMenuItem={SideMenuItemsEnum.DASHBOARD} />,
        children: [
          {
            path: URL_HOME.route,
            element: (
                <Home />
            ),
          },
        ],
      },
    ],
  },
  {
    element: <PublicRoute />,
    children: [
      {
        path: '*',
        element: <Navigate to={URL_LOGIN.route} />,
      },
      {
        path: URL_LOGIN.route,
        element: (
            <Authorization>
              <SignInForm />
            </Authorization>
        ),
      },
      {
        path: URL_SIGN_UP.route,
        element: (
            <Authorization>
              <SignUpForm />
            </Authorization>
        ),
      },
      {
        path: URL_VERIFY_INFO.route,
        element: (
            <Authorization>
              <VerifyAccountInfo />
            </Authorization>
        ),
      },
      {
        path: URL_VERIFY.route,
        element: (
            <Authorization>
              <VerifyAccount />
            </Authorization>
        ),
      },
    ],
  },
]);
