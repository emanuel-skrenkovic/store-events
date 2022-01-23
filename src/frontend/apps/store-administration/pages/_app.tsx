import { AppProps } from 'next/app';
import Head from 'next/head';
import { Provider } from 'react-redux';

import { productsStore } from '@store/products';

import './styles.css';

function CustomApp({ Component, pageProps }: AppProps) {
  return (
    <>
      <Head>
        <title>Welcome to store-administration!</title>
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/semantic-ui/2.4.1/semantic.min.css" />
      </Head>
      <main className="app">
        <Provider store={productsStore}>
          <Component {...pageProps} />
        </Provider>
      </main>
    </>
  );
}

export default CustomApp;
