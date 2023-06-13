import React, { Component, useState } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import AuthContextProvider from './contexts/AuthContext';
import './custom.css';

export default function App() {

    return (
      <Layout>
        <AuthContextProvider>
          <Routes>
            {AppRoutes.map((route, index) => {
              const { element, ...rest } = route;
              return <Route key={index} {...rest} element={element} />;
            })}
          </Routes>
        </AuthContextProvider>
      </Layout>
    )
}
