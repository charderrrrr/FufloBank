const API_URL = 'http://localhost:5002/api';

function getCurrentUser() {
    const user = sessionStorage.getItem('currentUser');
    return user ? JSON.parse(user) : null;
}

function setCurrentUser(user) {
    sessionStorage.setItem('currentUser', JSON.stringify(user));
}

function clearCurrentUser() {
    sessionStorage.removeItem('currentUser');
}

function isAuthenticated() {
    return getCurrentUser() !== null;
}

function redirectToLogin() {
    window.location.href = '/login.html';
}

function redirectToBalance() {
    window.location.href = '/balance.html';
}

async function login(phone) {
    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ phone: phone })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Авторизация не пройдена');
        }

        const user = await response.json();
        setCurrentUser(user);
        return user;
    } catch (error) {
        throw error;
    }
}

async function register(fullName, phone) {
    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName: fullName, phone: phone })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Регистрация не пройдена . _.');
        }

        const user = await response.json();
        setCurrentUser(user);
        return user;
    } catch (error) {
        throw error;
    }
}

async function logout() {
    try {
        await fetch(`${API_URL}/auth/logout`, { method: 'POST' });
    } finally {
        clearCurrentUser();
        window.location.href = '/login.html';
    }
}

async function getBalances() {
    const response = await fetch(`${API_URL}/balance`, {
        headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        throw new Error('Не удалось получить баланс');
    }

    return await response.json();
}

async function makePayment(currency, amount, mccCode, description) {
    const response = await fetch(`${API_URL}/payment`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            currency: parseInt(currency),
            amount: parseFloat(amount),
            mcc_code: parseInt(mccCode),
            description: description
        })
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        const error = await response.json();
        throw new Error(error.error || 'Оплата не прошла');
    }

    return await response.json();
}

async function makeTransfer(recipientPhone, currency, amount, confirmationCode) {
    const cleanPhone = recipientPhone.startsWith('+') ? recipientPhone : '+' + recipientPhone;
    
    const body = {
        recipientPhone: cleanPhone,
        currency: parseInt(currency),
        amount: parseFloat(amount),
        confirmationCode: confirmationCode || null
    };

    const response = await fetch(`${API_URL}/transfer`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        const error = await response.json();
        throw new Error(error.error || 'Перевод не прошел');
    }

    return await response.json();
}

async function checkTransferLimit(amount) {
    const response = await fetch(`${API_URL}/transfer/check-limit?amount=${amount}`, {
        headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        throw new Error('Не удалось проверить лимит');
    }

    return await response.json();
}

async function convertCurrency(fromCurrency, toCurrency, amount) {
    const response = await fetch(`${API_URL}/conversion`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            fromCurrency: parseInt(fromCurrency),
            toCurrency: parseInt(toCurrency),
            amount: parseFloat(amount)
        })
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        const error = await response.json();
        throw new Error(error.error || 'Перевод в другую валюту не удался');
    }

    return await response.json();
}

async function getHistory(fromDate, toDate, filter, category, minAmount, maxAmount) {
    let url = `${API_URL}/history?from_date=${fromDate}&to_date=${toDate}&filter=${filter}`;
    
    if (category) url += `&category=${category}`;
    if (minAmount) url += `&min_amount=${minAmount}`;
    if (maxAmount) url += `&max_amount=${maxAmount}`;

    const response = await fetch(url, {
        headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
        if (response.status === 401) redirectToLogin();
        throw new Error('Не удалось получить историю');
    }

    return await response.json();
}